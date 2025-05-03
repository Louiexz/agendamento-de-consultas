package com.example.unitsaude.activities;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;

import com.example.unitsaude.R;
import com.example.unitsaude.MainActivity;
import com.example.unitsaude.utils.SharedPreferencesManager;
import com.example.unitsaude.viewmodel.LoginViewModel;

public class LoginActivity extends AppCompatActivity {

    private EditText emailEditText, senhaEditText;
    private Button loginButton;
    private LoginViewModel loginViewModel;

    private ProgressBar progressBar;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);


        emailEditText = findViewById(R.id.emailEditText);
        senhaEditText = findViewById(R.id.senhaEditText);
        loginButton = findViewById(R.id.loginButton);
        progressBar = findViewById(R.id.progressBar);
        TextView esqueciSenhaText = findViewById(R.id.esqueciSenhaText);


        // Inicializa a ViewModel corretamente
        loginViewModel = new ViewModelProvider(this).get(LoginViewModel.class);

        // Observa o estado de carregamento
        loginViewModel.getLoadingLiveData().observe(this, isLoading -> {
            // Você pode adicionar um spinner de carregamento aqui
            progressBar.setVisibility(isLoading ? View.VISIBLE : View.GONE);
            loginButton.setEnabled(!isLoading);
        });

        // Observa o erro, se houver
        loginViewModel.getErrorLiveData().observe(this, errorMessage -> {
            if (errorMessage != null) {
                Toast.makeText(LoginActivity.this, errorMessage, Toast.LENGTH_SHORT).show();
            }
        });

        // Observa o token, indicando que o login foi bem-sucedido
        loginViewModel.getTokenLiveData().observe(this, token -> {
            if (token != null && !token.isEmpty()) {
                Intent intent = new Intent(LoginActivity.this, MainActivity.class);

                SharedPreferencesManager preferencesManager = new SharedPreferencesManager(this);
                String tipo = preferencesManager.getUserTipo();


            //    Intent intent;
             //   switch (tipo.toLowerCase()) {
            //        case "administrador":
                   //     intent = new Intent(this, AdminActivity.class);
                   //     break;
                  //  case "tecnico":
                   //     intent = new Intent(this, TecnicoActivity.class);
                   //     break;
                   // case "cliente":
                     //   intent = new Intent(this, ClienteActivity.class);
                    //    break;
            //        default:
             //           intent = new Intent(this, MainActivity.class);
               //         break;
             //   }

                startActivity(intent);
                finish();
                Toast.makeText(LoginActivity.this, "Login realizado com sucesso!", Toast.LENGTH_SHORT).show();
            }
        });

        // Configura o botão de login
        loginButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                String credential = emailEditText.getText().toString();
                String password = senhaEditText.getText().toString();

                if (credential.isEmpty()) {
                    emailEditText.setError("E-mail é obrigatório");
                    return;
                }

                if (!android.util.Patterns.EMAIL_ADDRESS.matcher(credential).matches() && !credential.matches("\\d+")) {
                    emailEditText.setError("Digite um e-mail válido");
                    return;
                }

                if (password.isEmpty()) {
                    senhaEditText.setError("Senha obrigatória");
                    return;
                }

                loginViewModel.login(credential, password);
            }
        });

        esqueciSenhaText.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(LoginActivity.this, RecuperarSenhaActivity.class);
                startActivity(intent);
            }
        });
    }
}
