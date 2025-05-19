package com.example.unitsaude.activities.consultation;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;

import java.util.ArrayList;
import java.util.List;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;

import com.example.unitsaude.activities.consultation.SelectAreaActivity;
import com.example.unitsaude.R;
import com.example.unitsaude.adapter.SelectSpecialtyAdapter;
import com.example.unitsaude.viewmodel.SelectSpecialtyView;

import android.widget.ListView;
import android.widget.ImageButton;
import android.widget.ProgressBar;

public class SelectSpecialtyActivity extends AppCompatActivity {
    private List<String> specialties;
    private SelectSpecialtyAdapter specialtiesAdapter;
    private SelectSpecialtyView specialtyViewModel;

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
        String area = getIntent().getStringExtra("selected_area");
        specialtyViewModel.getSpecialities(area); // <-- Substitua "area" pelo valor correto

        SelectSpecialtyAdapter adapter = new SelectSpecialtyAdapter(this, specialties, area);
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

                specialtiesAdapter = new SelectSpecialtyAdapter(this, specialties, area);

                listView.setAdapter(specialtiesAdapter);
            }
        });
    }
}