package com.example.unitsaude.activities.consultation;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.Toast;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.example.unitsaude.R;

public class AnamneseActivity extends AppCompatActivity {

    private EditText etAnamnese;
    private String selectedArea;
    private String selectedSpecialty;
    private int selectedProfessorId;
    private String selectedProfessorName;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_anamnese);

        // Inicializa componentes
        etAnamnese = findViewById(R.id.etAnamnese);
        Button btnProximo = findViewById(R.id.btnProximo);
        ImageButton btnVoltar = findViewById(R.id.btnVoltar);

        // Obtém dados da intent
        getIntentData();

        // Configura botão de voltar
        btnVoltar.setOnClickListener(v -> finish());

        // Configura botão próximo
        btnProximo.setOnClickListener(v -> {
            String anamneseText = etAnamnese.getText().toString().trim();

            if (validateAnamnese(anamneseText)) {
                goToSelectDateActivity(anamneseText);
            }
        });
    }

    private void getIntentData() {
        Intent intent = getIntent();
        selectedArea = intent.getStringExtra("selected_area");
        selectedSpecialty = intent.getStringExtra("selected_specialty");
        selectedProfessorId = intent.getIntExtra("selected_professor_id", -1); // Receba como int
        selectedProfessorName = intent.getStringExtra("selected_professor_name");
    }

    private boolean validateAnamnese(String anamnese) {
        if (anamnese.isEmpty()) {
            Toast.makeText(this, "Por favor, descreva seus sintomas", Toast.LENGTH_SHORT).show();
            return false;
        }

        if (anamnese.length() > 200) {
            Toast.makeText(this, "A descrição deve ter no máximo 200 caracteres", Toast.LENGTH_SHORT).show();
            return false;
        }

        return true;
    }

    private void goToSelectDateActivity(String anamnese) {
        Intent intent = new Intent(this, SelectDateActivity.class);

        // Passa todos os dados necessários para a próxima tela
        intent.putExtra("selected_area", selectedArea);
        intent.putExtra("selected_specialty", selectedSpecialty);
        intent.putExtra("selected_professor_id", selectedProfessorId);
        intent.putExtra("selected_professor_name", selectedProfessorName);
        intent.putExtra("anamnese", anamnese);

        startActivity(intent);
    }
}