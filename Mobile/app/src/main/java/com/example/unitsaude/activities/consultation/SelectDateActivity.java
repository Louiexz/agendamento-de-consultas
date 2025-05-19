package com.example.unitsaude.activities.consultation;

import android.content.Intent;
import android.os.Bundle;

import android.view.View;
import android.widget.ListView;
import android.widget.ImageButton;

import androidx.appcompat.app.AppCompatActivity;
import com.example.unitsaude.R;

import com.example.unitsaude.activities.consultation.SelectSpecialtyActivity;

public class SelectDateActivity extends AppCompatActivity {
    private ListView listView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_select_date);

        ImageButton backButton = findViewById(R.id.btnVoltar);
        ListView listView = findViewById(R.id.lvDate);

        String selectedArea = getIntent().getStringExtra("selected_area");

        backButton.setOnClickListener(v -> {
            Intent intent = new Intent(SelectDateActivity.this, SelectSpecialtyActivity.class);
            intent.putExtra("selected_area", selectedArea);
            startActivity(intent);
            finish();
        });
    }
}