package com.example.unitsaude.activities.consultation;

import android.app.DatePickerDialog;
import android.content.Intent;
import android.content.Context;
import android.os.Bundle;

import android.view.LayoutInflater;
import android.view.View;
import android.widget.Button;
import android.widget.GridLayout;
import android.widget.ImageButton;
import android.widget.ListView;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.CalendarView;
import android.widget.Toast;
import android.app.AlertDialog;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;

import com.example.unitsaude.R;

import com.example.unitsaude.data.dto.consultation.GetHourRequest;
import com.example.unitsaude.data.dto.consultation.GetHourDto;
import com.example.unitsaude.data.dto.consultation.GetHourResponse;

import com.example.unitsaude.data.dto.professor.GetProfessorDto;

import com.example.unitsaude.data.dto.consultation.GetConsultationDto;
import com.example.unitsaude.data.dto.consultation.CreateConsultationResponse;
import com.example.unitsaude.data.dto.consultation.CreateConsultationRequest;

import com.example.unitsaude.viewmodel.SelectHourView;
import com.example.unitsaude.viewmodel.CreateConsultationView;
import com.example.unitsaude.MainActivity;

import com.example.unitsaude.utils.SharedPreferencesManager;
import com.example.unitsaude.utils.MesesDoAno;

import com.google.android.material.bottomsheet.BottomSheetDialog;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;
import java.util.Locale;

public class SelectDateActivity extends AppCompatActivity {
    private String formattedDate;
    private String selected_hour;
    private String consult_status;
    private String selectedArea;
    private String selectedSpecialty;
    private String selectedProfessorName;
    private int user_id;
    private int selectedProfessorId;
    private String[] dateList = new String[3];
    
    private SelectHourView hourViewModel;
    private ProgressBar progressBarHours;
    private Context context;
    private List<GetHourDto> hourList = new ArrayList<>();
    private AlertDialog dialog;
    private CreateConsultationView createConsultationViewModel;
    private SharedPreferencesManager preferencesManager;

    private void formatDate(String date) {
        String mesNumero = date.substring(5, 7);
        String mesNome = MesesDoAno.obterMapaMeses().get(mesNumero);

        dateList[0] = date.substring(8, 10);
        dateList[1] = mesNome;
        dateList[2] = date.substring(0, 4);
    }

    private void showConfirmationDialog(GetConsultationDto consulta) {
        createConsultationViewModel.getLoadingLiveData().observe(this, isLoading -> {
            // Você pode adicionar um spinner de carregamento aqui
            AlertDialog loadingDialog = new AlertDialog.Builder(this)
                .setTitle("Carregando consulta...")
                .setView(new ProgressBar(this))
                .setCancelable(false)
                .create();
            

            AlertDialog confirmationDialog = new AlertDialog.Builder(this)
                .setTitle("Confirmação da consulta")
                .setMessage(
                    "Área: " + consulta.getArea() + "\n" +
                    "Especialidade: " + consulta.getEspecialidade() + "\n" +
                    "Data: " + dateList[0] + " de " +
                    dateList[1] + " de " + dateList[2] + "\n" +
                    "Professor: " + consulta.getProfessorName() + "\n" +
                    "Horário: " + consulta.getHora() + "\n" +
                    "Status: " + consulta.getStatus())
                .setPositiveButton("OK", (dialog, which) -> {
                    // Ação ao confirmar a consulta
                    Intent intent = new Intent(this, MainActivity.class);
                    startActivity(intent);
                    finish();
                })
                .create();
            
            if (isLoading) {
                loadingDialog.show();
            } else {
                loadingDialog.dismiss();
                formatDate(consulta.getData());

                confirmationDialog.show();
            }
        });
    }

    protected void createConsult() {
        createConsultationViewModel.createConsulta(
            new CreateConsultationRequest(
                formattedDate,
                selected_hour,
                consult_status,
                selectedArea,
                selectedSpecialty,
                user_id,
                selectedProfessorId
            )
        );

        createConsultationViewModel.getConsultationResponseLiveData().observe(this, responseBody -> {
            if (responseBody != null) {
                showConfirmationDialog(responseBody.getConsulta());
            }
        });
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_select_date);

        preferencesManager = new SharedPreferencesManager(this);

        user_id = preferencesManager.getUserId();

        context = this;

        selectedArea = getIntent().getStringExtra("selected_area");
        selectedSpecialty = getIntent().getStringExtra("selected_specialty");
        selectedProfessorName = getIntent().getStringExtra("selected_professor_name");
        selectedProfessorId = getIntent().getIntExtra("selected_professor_id", -1);

        hourViewModel = new ViewModelProvider(this).get(SelectHourView.class);
        createConsultationViewModel = new ViewModelProvider(this).get(CreateConsultationView.class);

        hourViewModel.getHourResponseLiveData().observe(this, response -> {
            if (response != null && response.getHours() != null && !response.getHours().isEmpty()) {
                hourList = response.getHours();
                showHourPickerDialog(hourList);
            } else {
                Toast.makeText(this, "Nenhum horário disponível para a data selecionada", Toast.LENGTH_SHORT).show();
            }
        });

        ImageButton backButton = findViewById(R.id.btnVoltar);
        backButton.setOnClickListener(v -> {
            Intent intent = new Intent(this, SelectSpecialtyActivity.class);
            intent.putExtra("selected_area", selectedArea);
            startActivity(intent);
            finish();
        });

        CalendarView calendarView = findViewById(R.id.calendarView);

        calendarView.setOnDateChangeListener((view, year, month, dayOfMonth) -> {
            Calendar selectedDate = Calendar.getInstance();
            selectedDate.set(year, month, dayOfMonth);

            SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault());
            String formattedDate = dateFormat.format(selectedDate.getTime());

            this.formattedDate = formattedDate; // Armazena a data formatada

            formatDate(formattedDate);

            GetHourRequest request = new GetHourRequest(formattedDate, selectedArea, selectedSpecialty);
            hourViewModel.getHours(request);
        });
    }

    private void showOptionsDialog() {
        AlertDialog optionsDialog = new AlertDialog.Builder(this)
            .setTitle("Confirmação")
            .setMessage(
                "Você deseja confirmar a consulta?\n\n" +
                "Área: " + selectedArea + "\n" +
                "Especialidade: " + selectedSpecialty + "\n" +
                "Professor: " + selectedProfessorName + "\n" +
                "Data: " + dateList[0] + " de " +
                dateList[1] + " de " + dateList[2] + "\n" +
                "Hora: " + selected_hour + "\n" +
                "Status: " + consult_status
            )
            .setPositiveButton("Sim", (dialog, which) -> {
                // Ação ao confirmar a consulta
                createConsult();
            })
            .setNegativeButton("Não", (dialog, which) -> dialog.dismiss())
            .create();
        
        optionsDialog.show();
    }    

    private void showHourPickerDialog(List<GetHourDto> hours) {
        BottomSheetDialog dialog = new BottomSheetDialog(context);
        View view = LayoutInflater.from(context).inflate(R.layout.dialog_time, null);

        Button btnCancel = view.findViewById(R.id.btnCancel);
        btnCancel.setOnClickListener(v -> dialog.dismiss());

        progressBarHours = view.findViewById(R.id.progressBarHours);

        hourViewModel.getLoadingLiveData().observe(this, isLoading -> {
            progressBarHours.setVisibility(isLoading ? View.VISIBLE : View.GONE);
        });

        GridLayout grid = view.findViewById(R.id.gridTimes);
        grid.removeAllViews();

        for (GetHourDto hour : hours) {
            Button button = new Button(context);
            button.setText(hour.getHora());

            if(hour.getStatus().equals("Disponível")) {
                button.setBackgroundResource(R.drawable.button_background_available);
            } else {
                button.setBackgroundResource(R.drawable.button_background_unavailable);
            }

            button.setOnClickListener(v -> {
                selected_hour = hour.getHora(); // Armazena a hora selecionada
                if (hour.getStatus().equals("Disponível")) {
                    consult_status = "Pendente";
                } else {
                    consult_status = "Em Espera";
                }
                showOptionsDialog(); // Chama o método de confirmação
            });

            GridLayout.LayoutParams params = new GridLayout.LayoutParams();
            params.setMargins(16, 16, 16, 16);
            grid.addView(button, params);
        }

        dialog.setContentView(view);
        dialog.show();
    }
}
