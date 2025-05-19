package com.example.unitsaude.activities.consultation;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;

import java.util.ArrayList;
import java.util.List;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;

import com.example.unitsaude.activities.consultation.SelectAreaActivity;
import com.example.unitsaude.activities.consultation.SelectDateActivity;
import com.example.unitsaude.R;
import com.example.unitsaude.adapter.SelectSpecialtyAdapter;
import com.example.unitsaude.viewmodel.SelectSpecialtyView;

import com.example.unitsaude.data.dto.professor.GetProfessorDto;
import com.example.unitsaude.data.dto.professor.GetProfessorResponse;

import android.view.LayoutInflater;
import android.view.View;

import android.widget.ArrayAdapter;
import android.widget.Spinner;
import android.widget.ListView;
import android.widget.ImageButton;
import android.widget.ProgressBar;

import androidx.lifecycle.ViewModelProvider;

import com.example.unitsaude.viewmodel.ProfessorView;

import android.app.AlertDialog;

public class SelectSpecialtyActivity extends AppCompatActivity {
    private List<String> specialties;
    private SelectSpecialtyAdapter specialtiesAdapter;
    private SelectSpecialtyView specialtyViewModel;
    private String selectedArea;
    private String selectedSpecialty;
    private ProfessorView professorViewModel; 

    public void getProfessorDialog() {
        AlertDialog loadingDialog = new AlertDialog.Builder(this)
            .setTitle("Carregando professores...")
            .setView(new ProgressBar(this))
            .setCancelable(false)
            .create();

        loadingDialog.show();

        professorViewModel = new ViewModelProvider(this).get(ProfessorView.class);

        // Remove observadores anteriores para evitar chamadas duplicadas
        professorViewModel.getProfessorResponseLiveData().removeObservers(this);

        // Solicita os dados
        professorViewModel.getProfessoresPorEspecialidade(selectedSpecialty);

        // Observa o resultado
        professorViewModel.getProfessorResponseLiveData().observe(this, responseBody -> {
            loadingDialog.dismiss();

            if (responseBody != null) {
                List<GetProfessorDto> professors = responseBody.getProfessores();
                if (professors == null || professors.isEmpty()) {
                    new AlertDialog.Builder(this)
                        .setTitle("Nenhum professor encontrado")
                        .setMessage("Não há professores disponíveis para essa especialidade.")
                        .setPositiveButton("OK", null)
                        .show();
                    return;
                }

                String[] professorNames = new String[professors.size()];
                for (int i = 0; i < professors.size(); i++) {
                    professorNames[i] = professors.get(i).getNome();
                }

                LayoutInflater inflater = LayoutInflater.from(this);
                View dialogView = inflater.inflate(R.layout.dialog_select_professor, null);
                Spinner spinner = dialogView.findViewById(R.id.spinnerProfessores);

                // Adapta o array de nomes ao spinner
                ArrayAdapter<String> adapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_dropdown_item, professorNames);
                spinner.setAdapter(adapter);

                new AlertDialog.Builder(this)
                    .setTitle("Selecione um professor")
                    .setView(dialogView)
                    .setPositiveButton("Confirmar", (dialog, which) -> {
                        int selectedIndex = spinner.getSelectedItemPosition();
                        GetProfessorDto selectedProfessor = professors.get(selectedIndex);

                        Intent intent = new Intent(this, SelectDateActivity.class);

                        intent.putExtra("selected_professor_name", selectedProfessor.getNome());
                        intent.putExtra("selected_professor_id", selectedProfessor.getId());
                        intent.putExtra("selected_specialty", selectedSpecialty);
                        intent.putExtra("selected_area", selectedArea);

                        startActivity(intent);
                    })
                    .setNegativeButton("Cancelar", null)
                    .show();
            } else {
                new AlertDialog.Builder(this)
                    .setTitle("Erro")
                    .setMessage("Falha ao carregar os professores.")
                    .setPositiveButton("OK", null)
                    .show();
            }
        });
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_select_specialty);

        ImageButton backButton = findViewById(R.id.btnVoltar);
        ListView listView = findViewById(R.id.lvSpecialty);
        ProgressBar progressBarSpecialities = findViewById(R.id.progressBarSpecialities);

        specialties = new ArrayList<>();

        specialtyViewModel = new ViewModelProvider(this).get(SelectSpecialtyView.class);

        // Obtenha a área selecionada da Intent
        selectedArea = getIntent().getStringExtra("selected_area");
        specialtyViewModel.getSpecialities(selectedArea); // <-- Substitua "selectedArea" pelo valor correto

        SelectSpecialtyAdapter adapter = new SelectSpecialtyAdapter(this, specialties, selectedArea, selectedSpecialty -> {
            this.selectedSpecialty = selectedSpecialty;
            getProfessorDialog();
        });
        listView.setAdapter(adapter);

        backButton.setOnClickListener(v -> {
            Intent intent = new Intent(SelectSpecialtyActivity.this, SelectAreaActivity.class);
            startActivity(intent);
            finish();
        });

        specialtyViewModel.getLoadingLiveData().observe(this, isLoading -> {
            // Você pode adicionar um spinner de carregamento aqui
            progressBarSpecialities.setVisibility(isLoading ? View.VISIBLE : View.GONE);
        });

        specialtyViewModel.getSpecialtyResponseLiveData().observe(this, responseBody -> {
            if (responseBody != null) {
                specialties = responseBody.getSpecialities();
                if (specialties == null) specialties = new ArrayList<>(); // <-- Adicione esta linha

                specialtiesAdapter = new SelectSpecialtyAdapter(this, specialties, selectedArea, selectedSpecialty -> {
                    this.selectedSpecialty = selectedSpecialty;
                    getProfessorDialog();
                });

                listView.setAdapter(specialtiesAdapter);
            }
        });
    }
}