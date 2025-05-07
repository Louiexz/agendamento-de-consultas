package com.example.unitsaude.activities;

import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;

import com.example.unitsaude.R;
import com.example.unitsaude.viewmodel.RecuperarSenhaViewModel;


public class RecuperarSenhaActivity extends AppCompatActivity {

    private EditText emailEditText;
    private Button enviarButton;
    private ProgressBar progressBar;
    private TextView erroText, sucessoText;
    private RecuperarSenhaViewModel viewModel;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        EdgeToEdge.enable(this);
        setContentView(R.layout.activity_recuperar_senha);

        // Inicializando os componentes da UI
        emailEditText = findViewById(R.id.emailEditText);
        enviarButton = findViewById(R.id.enviarButton);
        progressBar = findViewById(R.id.progressBar);
        erroText = findViewById(R.id.erroText);
        sucessoText = findViewById(R.id.sucessoText);

        // Inicializando o ViewModel
        viewModel = new ViewModelProvider(this).get(RecuperarSenhaViewModel.class);

        // Observando as mudanças nos dados do ViewModel
        observeViewModel();

        // Ação do botão de enviar
        enviarButton.setOnClickListener(v -> {
            String email = emailEditText.getText().toString().trim();
            if (email.isEmpty()) {
                emailEditText.setError("Informe o e-mail");
            } else {
                viewModel.recuperarSenha(email);
            }
        });
    }

    // Observando o estado do ViewModel
    private void observeViewModel() {
        // Observando o estado de loading
        viewModel.getLoadingLiveData().observe(this, isLoading -> {
            progressBar.setVisibility(isLoading ? View.VISIBLE : View.GONE);
            enviarButton.setEnabled(!isLoading);
        });

        // Observando a mensagem de sucesso
        viewModel.getSuccessLiveData().observe(this, mensagem -> {
            sucessoText.setVisibility(View.VISIBLE);
            sucessoText.setText(mensagem);
            erroText.setVisibility(View.GONE);
        });

        // Observando a mensagem de erro
        viewModel.getErrorLiveData().observe(this, erro -> {
            erroText.setVisibility(View.VISIBLE);
            erroText.setText(erro);
            sucessoText.setVisibility(View.GONE);
        });
    }
}