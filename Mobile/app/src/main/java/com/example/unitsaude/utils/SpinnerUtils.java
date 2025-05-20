package com.example.unitsaude.utils;

import android.content.Context;
import android.view.View;

import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Spinner;

import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.unitsaude.R;
import java.util.List;

public class SpinnerUtils {

    public static void setupSpinner(Context context, Spinner spinner, List<String> items, View rootView) {
        if (context == null || spinner == null || items == null) return;

        ArrayAdapter<String> adapter = new ArrayAdapter<>(context, android.R.layout.simple_spinner_item, items);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(adapter);
        setupSpinnerListener(spinner, rootView);
    }

    public static String getSelectedTipo(Spinner spinner) {
        return spinner != null ? (String) spinner.getSelectedItem() : null;
    }

    private static void mostrarSomente(View layoutVisivel, View rootView) {
        if (rootView == null || layoutVisivel == null) return;

        int[] layoutIds = {
            R.id.consultasAgendadas,
            R.id.consultasPendente,
            R.id.consultasEmEspera,
            R.id.consultasCanceladas,
            R.id.consultasConcluidas
        };

        for (int id : layoutIds) {
            View layout = rootView.findViewById(id);
            if (layout != null) {
                layout.setVisibility(layout == layoutVisivel ? View.VISIBLE : View.GONE);
            }
        }
    }

    public static void setupSpinnerListener(Spinner spinner, View rootView) {
        if (spinner == null || rootView == null) return;

        
    }
}