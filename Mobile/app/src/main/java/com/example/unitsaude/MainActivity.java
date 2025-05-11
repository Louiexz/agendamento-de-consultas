package com.example.unitsaude;

import android.os.Bundle;
import android.widget.ListView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.example.unitsaude.adapter.ConsultaAgendadaAdapter;
import com.example.unitsaude.adapter.ConsultaEsperaAdapter;

import com.example.unitsaude.data.dto.consultation.GetConsultationDto;
import com.example.unitsaude.data.dto.consultation.GetConsultationRequest;

import java.util.ArrayList;
import java.util.List;

import com.example.unitsaude.viewmodel.ConsultationView;
import android.view.View;
import androidx.lifecycle.ViewModelProvider;
import com.example.unitsaude.utils.SharedPreferencesManager;

public class MainActivity extends AppCompatActivity {
    private List<GetConsultationDto> consultas; // Lista de todas as consultas
    private ConsultaAgendadaAdapter agendadasAdapter;
    private ConsultaEsperaAdapter esperaAdapter;
    private ConsultationView consultationViewModel;
    private SharedPreferencesManager preferencesManager;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        preferencesManager = new SharedPreferencesManager(this);
        consultationViewModel = new ViewModelProvider(this).get(ConsultationView.class);

        consultas = new ArrayList<>();

        int id = preferencesManager.getUserId();
        consultationViewModel.getConsultas(id);

        ListView listViewAgendadas = findViewById(R.id.listViewAgendadas);
        ListView listViewEspera = findViewById(R.id.listViewEspera);

        if (listViewAgendadas == null || listViewEspera == null) {
            Toast.makeText(this, "Erro ao encontrar os ListViews", Toast.LENGTH_SHORT).show();
            return;
        }

        consultationViewModel.getConsultationResponseLiveData().observe(this, responseBody -> {
            if (responseBody != null) {
                consultas = responseBody.getConsultas();
                if (consultas == null) consultas = new ArrayList<>(); // <-- Adicione esta linha

                List<GetConsultationDto> consultasAgendadas = filterConsultasByStatus(consultas, "Agendada");
                List<GetConsultationDto> consultasEmEspera = filterConsultasByStatus(consultas, "Em Espera");

                agendadasAdapter = new ConsultaAgendadaAdapter(this, consultasAgendadas);
                esperaAdapter = new ConsultaEsperaAdapter(this, consultasEmEspera);

                listViewAgendadas.setAdapter(agendadasAdapter);
                listViewEspera.setAdapter(esperaAdapter);
            }
        });
    }

    // Método para filtrar consultas com base no status
    private List<GetConsultationDto> filterConsultasByStatus(List<GetConsultationDto> allConsultas, String status) {
        List<GetConsultationDto> filtered = new ArrayList<>();
        for (GetConsultationDto consulta : allConsultas) {
            if (consulta.getStatus() != null && consulta.getStatus().equals(status)) {
                filtered.add(consulta);
            }
        }
        return filtered;
    }
}
