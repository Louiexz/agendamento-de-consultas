package com.example.unitsaude.utils;

import android.content.Context;
import android.app.AlertDialog;
import android.view.View;
import android.view.LayoutInflater;

import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Spinner;

import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.unitsaude.R;
import java.util.List;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

public class SelectionDialogManager {
    private final Context context;
    private final List<String> options;
    private final Map<String, View> recyclerViewMap;
    private int lastSelectedPosition = 0; // Inicia com a primeira posição

    public SelectionDialogManager(Context context, Map<String, View> recyclerViewMap) {
        this.context = context;
        this.recyclerViewMap = new HashMap<>(recyclerViewMap); // Cópia defensiva
        this.options = new ArrayList<>(recyclerViewMap.keySet());
        
        // Garante visibilidade inicial correta
        if (!options.isEmpty()) {
            updateRecyclerVisibility(options.get(0));
        }
    }

    public void showSelectionDialog() {
        View dialogView = LayoutInflater.from(context).inflate(R.layout.select_tipo, null);
        Spinner spinner = dialogView.findViewById(R.id.spinnerTipos);

        ArrayAdapter<String> adapter = new ArrayAdapter<>(
            context,
            android.R.layout.simple_spinner_item,
            options
        );
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(adapter);

        // Restaura a seleção anterior ou usa a primeira opção
        int selection = lastSelectedPosition >= 0 && lastSelectedPosition < options.size() 
            ? lastSelectedPosition : 0;
        spinner.setSelection(selection);

        spinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                if (position >= 0 && position < options.size()) {
                    String selected = options.get(position);
                    lastSelectedPosition = position;
                    updateRecyclerVisibility(selected);
                }
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {
                // Mantém a seleção atual
            }
        });

        new AlertDialog.Builder(context)
            .setTitle("Selecione a visualização")
            .setView(dialogView)
            .setPositiveButton("OK", (dialog, which) -> dialog.dismiss())
            .setOnDismissListener(dialog -> {
                // Força atualização ao fechar o diálogo
                if (lastSelectedPosition >= 0 && lastSelectedPosition < options.size()) {
                    updateRecyclerVisibility(options.get(lastSelectedPosition));
                }
            })
            .show();
    }

    private void updateRecyclerVisibility(String selectedKey) {
        for (Map.Entry<String, View> entry : recyclerViewMap.entrySet()) {
            View recyclerView = entry.getValue();
            if (recyclerView == null) continue;
            
            boolean shouldBeVisible = entry.getKey().equals(selectedKey);
            recyclerView.setVisibility(shouldBeVisible ? View.VISIBLE : View.GONE);
        }
    }
}