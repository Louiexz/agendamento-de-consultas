package com.example.unitsaude;

import java.util.ArrayList;
import java.util.List;

import android.content.Intent;
import android.os.Bundle;

import android.widget.Spinner;
import android.widget.Toast;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.LinearLayout;

import androidx.appcompat.app.AppCompatActivity;

import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.unitsaude.activities.LoginActivity;

import com.example.unitsaude.adapter.ConsultaAgendadaAdapter;
import com.example.unitsaude.adapter.ConsultaEsperaAdapter;

import com.example.unitsaude.data.dto.consultation.GetConsultationDto;
import com.example.unitsaude.data.dto.consultation.GetConsultationRequest;

import com.example.unitsaude.viewmodel.ConsultationView;
import com.example.unitsaude.viewmodel.CancelConsultationView;

import android.view.LayoutInflater;
import android.view.View;

import androidx.lifecycle.ViewModelProvider;

import com.example.unitsaude.activities.consultation.SelectAreaActivity;

import com.example.unitsaude.utils.SharedPreferencesManager;
import com.example.unitsaude.utils.AlertDialogUtils;
import com.example.unitsaude.utils.SelectionDialogManager;

import java.util.HashMap;
import java.util.Map;

public class MainActivity extends AppCompatActivity {
    private List<GetConsultationDto> consultas; // Lista de todas as consultas
    private Button btnStatusConsulta;

    private ConsultaAgendadaAdapter agendadasAdapter;
    private ConsultaEsperaAdapter esperaAdapter;
    private ConsultaEsperaAdapter pendenteAdapter;
    private ConsultaAgendadaAdapter canceladaAdapter;
    private ConsultaAgendadaAdapter concluidaAdapter;

    private AlertDialogUtils alertDialogUtils;
    private SelectionDialogManager dialogManager;

    private SharedPreferencesManager preferencesManager;

    private ConsultationView consultationViewModel;
    private CancelConsultationView cancelConsultationViewModel;
    private int lastCancelledConsultaId = -1;

    private List<String> consultasTipos;

    private void showLoadingDialog(String title) {
        alertDialogUtils.showLoadingDialog(title);
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        alertDialogUtils = new AlertDialogUtils(this);

        preferencesManager = new SharedPreferencesManager(this);

        ImageButton btnSair = findViewById(R.id.btnSair);
        btnStatusConsulta = findViewById(R.id.btnStatusConsulta);
        LinearLayout btnAgendar = findViewById(R.id.btnAgendar);   

        // Initialize RecyclerView (if it's in the main layout)
        RecyclerView recyclerAgendada = findViewById(R.id.recyclerAgendadas);
        recyclerAgendada.setLayoutManager(new LinearLayoutManager(this));

        RecyclerView recyclerEspera = findViewById(R.id.recyclerEspera);
        recyclerEspera.setLayoutManager(new LinearLayoutManager(this));

        RecyclerView recyclerPendente = findViewById(R.id.recyclerPendente);
        recyclerPendente.setLayoutManager(new LinearLayoutManager(this));

        RecyclerView recyclerCancelada = findViewById(R.id.recyclerCancelada);
        recyclerCancelada.setLayoutManager(new LinearLayoutManager(this));

        RecyclerView recyclerConcluida = findViewById(R.id.recyclerConcluida);
        recyclerConcluida.setLayoutManager(new LinearLayoutManager(this));

        if (recyclerAgendada == null || recyclerEspera == null
            || recyclerPendente == null || recyclerCancelada == null
            || recyclerConcluida == null) {
            Toast.makeText(this, "Erro ao encontrar os RecyclerViews", Toast.LENGTH_SHORT).show();
            return;
        }

        // Mapear as opções para os RecyclerViews
        Map<String, View> recyclerMap = new HashMap<>();
        recyclerMap.put("Agendadas", findViewById(R.id.consultasAgendadas));
        recyclerMap.put("Em Espera", findViewById(R.id.consultasEmEspera));
        recyclerMap.put("Pendente", findViewById(R.id.consultasPendente));
        recyclerMap.put("Cancelada", findViewById(R.id.consultasCanceladas));
        recyclerMap.put("Concluída", findViewById(R.id.consultasConcluidas));

        dialogManager = new SelectionDialogManager(this, recyclerMap);

        btnStatusConsulta.setOnClickListener(v -> {
            dialogManager.showSelectionDialog();
        });

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
        cancelConsultationViewModel = new ViewModelProvider(this).get(CancelConsultationView.class);

        consultas = new ArrayList<>();

        int id = preferencesManager.getUserId();
        consultationViewModel.getConsultas(id);

        consultationViewModel.getLoadingLiveData().observe(this, isLoading -> {
            // Você pode adicionar um spinner de carregamento aqui
            if (isLoading) {
                alertDialogUtils.showLoadingDialog("Carregando consultas...");
            } else {
                alertDialogUtils.closeLoadingDialog();
            }
        });

        cancelConsultationViewModel.getConsultationResponseLiveData().observe(this, response -> {
            if (response != null) {
                alertDialogUtils.closeLoadingDialog();

                alertDialogUtils.showCancelConsultationDialog();
                
                // Remover a consulta da lista e atualizar o adapter
                esperaAdapter.removerConsultaPorId(lastCancelledConsultaId);
                pendenteAdapter.removerConsultaPorId(lastCancelledConsultaId);
            }
        });

        consultationViewModel.getConsultationResponseLiveData().observe(this, responseBody -> {
            if (responseBody != null) {
                consultas = responseBody.getConsultas();
                if (consultas == null) consultas = new ArrayList<>(); // <-- Adicione esta linha

                List<GetConsultationDto> consultasAgendadas = filterConsultasByStatus(consultas, "Agendada");
                List<GetConsultationDto> consultasEmEspera = filterConsultasByStatus(consultas, "Em Espera");
                List<GetConsultationDto> consultasPendente = filterConsultasByStatus(consultas, "Pendente");
                List<GetConsultationDto> consultasCanceladas = filterConsultasByStatus(consultas, "Cancelada");
                List<GetConsultationDto> consultasConcluidas = filterConsultasByStatus(consultas, "Concluída");

                agendadasAdapter = new ConsultaAgendadaAdapter(this, consultasAgendadas);

                esperaAdapter = new ConsultaEsperaAdapter(this, consultasEmEspera, 
                    consultaId -> {
                        lastCancelledConsultaId = consultaId;
                        cancelConsultationViewModel.cancelConsultation(consultaId);
                        showLoadingDialog("Cancelando consulta...");
                    });

                pendenteAdapter = new ConsultaEsperaAdapter(this, consultasPendente,
                    consultaId -> {
                        lastCancelledConsultaId = consultaId;
                        cancelConsultationViewModel.cancelConsultation(consultaId);
                        showLoadingDialog("Cancelando consulta...");
                    });
                
                canceladaAdapter = new ConsultaAgendadaAdapter(this, consultasCanceladas);
                concluidaAdapter = new ConsultaAgendadaAdapter(this, consultasConcluidas);

                recyclerAgendada.setAdapter(agendadasAdapter);
                recyclerEspera.setAdapter(esperaAdapter);
                recyclerPendente.setAdapter(pendenteAdapter);
                recyclerCancelada.setAdapter(canceladaAdapter);
                recyclerConcluida.setAdapter(concluidaAdapter);
            }
        });
        consultationViewModel.getErrorLiveData().observe(this, errorMessage -> {
            if (errorMessage != null) {
                Toast.makeText(this, errorMessage, Toast.LENGTH_SHORT).show();

                if (errorMessage.contains("Sessão expirada")) {
                    // Redirecionar para LoginActivity
                    Intent intent = new Intent(this, LoginActivity.class);
                    intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                    startActivity(intent);
                    finish(); // Fecha a activity atual
                }
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
