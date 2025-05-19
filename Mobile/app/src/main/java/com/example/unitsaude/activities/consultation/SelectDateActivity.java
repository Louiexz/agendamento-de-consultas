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
import com.example.unitsaude.viewmodel.SelectHourView;
import com.example.unitsaude.MainActivity;

import com.example.unitsaude.utils.MesesDoAno;

import com.google.android.material.bottomsheet.BottomSheetDialog;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;
import java.util.Locale;

public class SelectDateActivity extends AppCompatActivity {
    private String selected_hour;
    private String selectedArea;
    private String selectedSpecialty;
    private String formattedDate;
    private String[] dateList = new String[3];
    private SelectHourView hourViewModel;
    private ProgressBar progressBarHours;
    private Context context;
    private List<GetHourDto> hourList = new ArrayList<>();
    private AlertDialog dialog;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_select_date);

        context = this;

        selectedArea = getIntent().getStringExtra("selected_area");
        selectedSpecialty = getIntent().getStringExtra("selected_specialty");

        hourViewModel = new ViewModelProvider(this).get(SelectHourView.class);

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

            String mesNumero = formattedDate.substring(5, 7);
            String mesNome = MesesDoAno.obterMapaMeses().get(mesNumero);

            dateList[0] = formattedDate.substring(8, 10);
            dateList[1] = mesNome;
            dateList[2] = formattedDate.substring(0, 4);

            GetHourRequest request = new GetHourRequest(formattedDate, selectedArea, selectedSpecialty);
            hourViewModel.getHours(request);
        });
    }

    private void showConfirmationDialog() {
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle("Confirmação da consulta");
        builder.setMessage(
            "Área: " + selectedArea + "\n" +
            "Especialidade: " + selectedSpecialty + "\n" +
            "Data: " + dateList[0] + " de " +
            dateList[1] + " de " + dateList[2] + "\n" +
            "Horário: " + selected_hour);
        
        builder.setPositiveButton("OK", (dialog, which) -> {
            // Ação ao confirmar a consulta
            Intent intent = new Intent(this, MainActivity.class);
            startActivity(intent);
            finish();
        });

        AlertDialog dialog = builder.create();
        dialog.show();
    }

    private void showOptionsDialog() {
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle("Confirmação");
        builder.setMessage(
            "Você deseja confirmar a consulta?\n\n" +
            "Área: " + selectedArea + "\n" +
            "Especialidade: " + selectedSpecialty + "\n" +
            "Dia: " + formattedDate + "\n" +
            "Hora: " + selected_hour);

        builder.setPositiveButton("Sim", (dialog, which) -> {
            // Ação ao confirmar a consulta
            showConfirmationDialog();
        });

        builder.setNegativeButton("Não", (dialog, which) -> dialog.dismiss());

        AlertDialog dialog = builder.create();
        dialog.show();
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
