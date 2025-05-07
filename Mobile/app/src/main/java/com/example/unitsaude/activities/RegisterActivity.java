package com.example.unitsaude.activities;

import androidx.appcompat.app.AppCompatActivity;
import android.os.Bundle;
import com.example.unitsaude.activities.LoginActivity;
import android.content.Intent;

import com.example.unitsaude.R;
import android.widget.Toast;
import android.widget.EditText;
import android.widget.Button;
import android.widget.DatePicker;
import android.widget.ProgressBar;

import android.app.DatePickerDialog;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Locale;

import com.example.unitsaude.viewmodel.RegisterView;
import android.view.View;
import androidx.lifecycle.ViewModelProvider;

public class RegisterActivity extends AppCompatActivity {
    private RegisterView registerViewModel;
    private ProgressBar progressBar;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_register);

        EditText cpfEditText = findViewById(R.id.cpfEditText);
        EditText nomeEditText = findViewById(R.id.nomeEditText);
        EditText emailEditText = findViewById(R.id.emailEditText);
        EditText telefoneEditText = findViewById(R.id.telefoneEditText);
        EditText senhaEditText = findViewById(R.id.senhaEditText);
        EditText confirmaSenhaEditText = findViewById(R.id.confirmaSenhaEditText);
        EditText nascimentoEditText = findViewById(R.id.nascimentoEditText);
        
        progressBar = findViewById(R.id.progressBar);
        Button registrarButton = findViewById(R.id.registrarButton);
        Button voltarButton = findViewById(R.id.voltarButton);

        registerViewModel = new ViewModelProvider(this).get(RegisterView.class);

        // Observa o estado de carregamento
        registerViewModel.getLoadingLiveData().observe(this, isLoading -> {
            // Você pode adicionar um spinner de carregamento aqui
            progressBar.setVisibility(isLoading ? View.VISIBLE : View.GONE);
            registrarButton.setEnabled(!isLoading);
        });

        // Observa o erro, se houver
        registerViewModel.getErrorLiveData().observe(this, errorMessage -> {
            if (errorMessage != null) {
                Toast.makeText(RegisterActivity.this, errorMessage, Toast.LENGTH_SHORT).show();
            }
        });

        nascimentoEditText.setOnClickListener(v -> {
            Calendar calendar = Calendar.getInstance();
            int year = calendar.get(Calendar.YEAR);
            int month = calendar.get(Calendar.MONTH);
            int day = calendar.get(Calendar.DAY_OF_MONTH);

            // Cria o DatePickerDialog
            DatePickerDialog datePickerDialog = new DatePickerDialog(this, new DatePickerDialog.OnDateSetListener() {
                @Override
                public void onDateSet(DatePicker view, int selectedYear, int selectedMonth, int selectedDay) {
                    // Formato para mostrar a data
                    Calendar selectedDate = Calendar.getInstance();
                    selectedDate.set(selectedYear, selectedMonth, selectedDay);

                    SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault());
                    String formattedDate = dateFormat.format(selectedDate.getTime());
                    
                    // Atualiza o EditText com a data selecionada
                    nascimentoEditText.setText(formattedDate);
                }
            }, year, month, day);

            // Mostra o DatePickerDialog
            datePickerDialog.show();
        });

        voltarButton.setOnClickListener(v -> {
            startActivity(new Intent(RegisterActivity.this, LoginActivity.class));
            finish();
        });

        registrarButton.setOnClickListener(v -> {
            String cpf = cpfEditText.getText().toString().trim();
            String nome = nomeEditText.getText().toString().trim();
            String email = emailEditText.getText().toString().trim();
            String telefone = telefoneEditText.getText().toString().trim();
            String senha = senhaEditText.getText().toString().trim();
            String confirmaSenha = confirmaSenhaEditText.getText().toString().trim();
            String dataNascimento = nascimentoEditText.getText().toString().trim();

            if (cpf.isEmpty() || nome.isEmpty() || email.isEmpty() || senha.isEmpty()) {
                Toast.makeText(this, "Preencha todos os campos obrigatórios", Toast.LENGTH_SHORT).show();
                return;
            }

            if (nome.length() < 5 || !nome.contains(" ")) {
                nomeEditText.setError("Nome inválido. Digite nome e sobrenome.");
                return;
            }

            if (!android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
                emailEditText.setError("E-mail inválido");
                return;
            }

            if (cpf.length() != 11 || !cpf.matches("\\d+")) {
                cpfEditText.setError("CPF inválido");
                return;
            }

            if (senha.length() < 6) {
                senhaEditText.setError("A senha deve ter pelo menos 6 caracteres");
                return;
            }

            if (!senha.equals(confirmaSenha)) {
                Toast.makeText(this, "As senhas não coincidem", Toast.LENGTH_SHORT).show();
                return;
            }

            registerViewModel.registrar(cpf, nome, email, senha, telefone, dataNascimento);
        });

        registerViewModel.getRegisterResponseLiveData().observe(this, responseBody -> {
            if (responseBody != null) {
                Toast.makeText(RegisterActivity.this, "Cadastro realizado com sucesso", Toast.LENGTH_SHORT).show();
                startActivity(new Intent(RegisterActivity.this, LoginActivity.class));
                finish();
            }
        });
    }
}