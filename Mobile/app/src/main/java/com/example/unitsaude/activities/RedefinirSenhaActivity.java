package com.example.unitsaude.activities;

import android.content.Intent; // Importação necessária para usar a classe Intent

import android.net.Uri;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;

import com.example.unitsaude.R;
import com.example.unitsaude.viewmodel.RedefinirSenhaViewModel;

public class RedefinirSenhaActivity extends AppCompatActivity {

    private RedefinirSenhaViewModel viewModel;
    private EditText editTextNovaSenha, editTextConfirmarSenha;
    private Button btnRedefinirSenha;
    private ProgressBar progressBar;
    private TextView erroText, sucessoText;
    private String token; // Token recebido via Intent

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_redefinir_senha);

        // 🔄 Corrigido: recupera o token da URL (deep link)
        Uri data = getIntent().getData();
        if (data != null && data.getQueryParameter("token") != null) {
            token = data.getQueryParameter("token");
        } else {
            Toast.makeText(this, "Token inválido ou ausente.", Toast.LENGTH_LONG).show();
            finish();
            return;
        }

        // Inicializa os componentes da interface
        editTextNovaSenha = findViewById(R.id.novaSenhaEditText);
        editTextConfirmarSenha = findViewById(R.id.confirmarSenhaEditText);
        btnRedefinirSenha = findViewById(R.id.redefinirButton);
        progressBar = findViewById(R.id.progressBar);
        erroText = findViewById(R.id.erroText);
        sucessoText = findViewById(R.id.sucessoText);

        // ViewModel
        viewModel = new ViewModelProvider(this, new ViewModelProvider.AndroidViewModelFactory(getApplication()))
                .get(RedefinirSenhaViewModel.class);

        // Observa loading
        viewModel.getLoadingLiveData().observe(this, isLoading -> {
            progressBar.setVisibility(isLoading ? View.VISIBLE : View.GONE);
            btnRedefinirSenha.setEnabled(!isLoading);
        });

        // Observa sucesso
        viewModel.getSuccessLiveData().observe(this, successMessage -> {
            sucessoText.setText(successMessage);
            sucessoText.setVisibility(View.VISIBLE);
            erroText.setVisibility(View.GONE);
            Toast.makeText(this, successMessage, Toast.LENGTH_LONG).show();

            Intent intent = new Intent(RedefinirSenhaActivity.this, LoginActivity.class);
            startActivity(intent);

            finish(); // ou redirecionar para login, se preferir
        });

        // Observa erro
        viewModel.getErrorLiveData().observe(this, errorMessage -> {
            erroText.setText(errorMessage);
            erroText.setVisibility(View.VISIBLE);
            sucessoText.setVisibility(View.GONE);
        });

        // Clique no botão
        btnRedefinirSenha.setOnClickListener(view -> {
            String novaSenha = editTextNovaSenha.getText().toString().trim();
            String confirmarSenha = editTextConfirmarSenha.getText().toString().trim();

            if (novaSenha.isEmpty() || confirmarSenha.isEmpty()) {
                erroText.setText("Por favor, preencha todos os campos.");
                erroText.setVisibility(View.VISIBLE);
                sucessoText.setVisibility(View.GONE);
                return;
            }

            if (!novaSenha.equals(confirmarSenha)) {
                erroText.setText("As senhas não coincidem.");
                erroText.setVisibility(View.VISIBLE);
                sucessoText.setVisibility(View.GONE);
                return;
            }

            viewModel.redefinirSenha(token, novaSenha, confirmarSenha);
        });
    }
}
