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

import android.app.DatePickerDialog;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Locale;

public class RegisterActivity extends AppCompatActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_register);

        EditText cpfEditText = findViewById(R.id.cpfEditText);
        EditText nomeEditText = findViewById(R.id.nomeEditText);
        EditText emailEditText = findViewById(R.id.emailEditText);
        EditText senhaEditText = findViewById(R.id.senhaEditText);
        EditText confirmaSenhaEditText = findViewById(R.id.confirmaSenhaEditText);
        Button registrarButton = findViewById(R.id.registrarButton);
        Button voltarButton = findViewById(R.id.voltarButton);

        EditText nascimentoEditText = findViewById(R.id.nascimentoEditText);

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

                    SimpleDateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy", Locale.getDefault());
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
            String senha = senhaEditText.getText().toString().trim();
            String confirmaSenha = confirmaSenhaEditText.getText().toString().trim();

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

            // Aqui você pode chamar a ViewModel ou um serviço de API para registrar o usuário
            Toast.makeText(this, "Registro enviado!", Toast.LENGTH_SHORT).show();
        });
    }
}