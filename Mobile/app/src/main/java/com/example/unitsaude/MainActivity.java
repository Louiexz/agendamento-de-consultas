package com.example.unitsaude;

import java.util.ArrayList;
import java.util.List;

import android.content.Intent;
import android.os.Bundle;

import android.widget.ListView;
import android.widget.Toast;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.LinearLayout;

import androidx.appcompat.app.AppCompatActivity;

import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.example.unitsaude.activities.LoginActivity;

import com.example.unitsaude.adapter.ConsultaAgendadaAdapter;
import com.example.unitsaude.adapter.ConsultaEsperaAdapter;

import com.example.unitsaude.data.dto.consultation.GetConsultationDto;
import com.example.unitsaude.data.dto.consultation.GetConsultationRequest;

import com.example.unitsaude.viewmodel.ConsultationView;
import android.view.View;
import androidx.lifecycle.ViewModelProvider;
import com.example.unitsaude.utils.SharedPreferencesManager;

import com.example.unitsaude.activities.consultation.SelectAreaActivity;

public class MainActivity extends AppCompatActivity {
    private List<GetConsultationDto> consultas; // Lista de todas as consultas
    private ConsultaAgendadaAdapter agendadasAdapter;
    private ConsultaEsperaAdapter esperaAdapter;
    private ConsultaEsperaAdapter pendenteAdapter;
    private ConsultationView consultationViewModel;
    private SharedPreferencesManager preferencesManager;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        preferencesManager = new SharedPreferencesManager(this);

        ImageButton btnSair = findViewById(R.id.btnSair);
        LinearLayout btnAgendar = findViewById(R.id.btnAgendar);
        ProgressBar progressBarAgendadas = findViewById(R.id.progressBarAgendadas);
        ProgressBar progressBarEspera = findViewById(R.id.progressBarEspera);
        ProgressBar progressBarPendente = findViewById(R.id.progressBarPendente);

        // Configura o botão de agendar
        btnAgendar.setOnClickListener(v -> {
            Intent intent = new Intent(MainActivity.this, SelectAreaActivity.class);
            startActivity(intent);
        });

        // Configura o botão de sair
        btnSair.setOnClickListener(v -> {
            preferencesManager.clearAll();

            Intent intent = new Intent(MainActivity.this, LoginActivity.class);
            startActivity(intent);
            finish();
        });

        // Verifica se o usuário está logado
        if (preferencesManager.isLoggedIn()) {
            com.example.unitsaude.data.api.ApiClient.setAuthToken(preferencesManager.getAuthToken());
        }

        consultationViewModel = new ViewModelProvider(this).get(ConsultationView.class);

        consultas = new ArrayList<>();

        int id = preferencesManager.getUserId();
        consultationViewModel.getConsultas(id);

        ListView listViewAgendadas = findViewById(R.id.listViewAgendadas);
        ListView listViewEspera = findViewById(R.id.listViewEspera);
        ListView listViewPendente = findViewById(R.id.listViewPendente);

        if (listViewAgendadas == null || listViewEspera == null) {
            Toast.makeText(this, "Erro ao encontrar os ListViews", Toast.LENGTH_SHORT).show();
            return;
        }

        consultationViewModel.getLoadingLiveData().observe(this, isLoading -> {
            // Você pode adicionar um spinner de carregamento aqui
            progressBarAgendadas.setVisibility(isLoading ? View.VISIBLE : View.GONE);
            progressBarEspera.setVisibility(isLoading ? View.VISIBLE : View.GONE);
            progressBarPendente.setVisibility(isLoading ? View.VISIBLE : View.GONE);
        });

        consultationViewModel.getConsultationResponseLiveData().observe(this, responseBody -> {
            if (responseBody != null) {
                consultas = responseBody.getConsultas();
                if (consultas == null) consultas = new ArrayList<>(); // <-- Adicione esta linha

                List<GetConsultationDto> consultasAgendadas = filterConsultasByStatus(consultas, "Agendada");
                List<GetConsultationDto> consultasEmEspera = filterConsultasByStatus(consultas, "Em Espera");
                List<GetConsultationDto> consultasPendente = filterConsultasByStatus(consultas, "Pendente");

                agendadasAdapter = new ConsultaAgendadaAdapter(this, consultasAgendadas);
                esperaAdapter = new ConsultaEsperaAdapter(this, consultasEmEspera);
                pendenteAdapter = new ConsultaEsperaAdapter(this, consultasPendente);

                listViewAgendadas.setAdapter(agendadasAdapter);
                listViewEspera.setAdapter(esperaAdapter);
                listViewPendente.setAdapter(pendenteAdapter);
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
