package com.example.unitsaude.activities.consultation;

import androidx.appcompat.app.AppCompatActivity;
import android.content.Intent;
import android.os.Bundle;

import com.example.unitsaude.adapter.SelectAreaAdapter;
import com.example.unitsaude.R;
import com.example.unitsaude.MainActivity;

import android.widget.ListView;
import android.widget.ImageButton;

public class SelectAreaActivity extends AppCompatActivity {
    private ListView listView;
    private SelectAreaAdapter adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_select_area);

        ImageButton backButton = findViewById(R.id.btnVoltar);
        ListView listView = findViewById(R.id.lvArea);

        backButton.setOnClickListener(v -> {
            Intent intent = new Intent(SelectAreaActivity.this, MainActivity.class);
            startActivity(intent);
            finish();
        });

        adapter = new SelectAreaAdapter(this);
        listView.setAdapter(adapter);
    }
}