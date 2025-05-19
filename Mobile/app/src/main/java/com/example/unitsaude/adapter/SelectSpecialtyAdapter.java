package com.example.unitsaude.adapter;

import android.content.Context;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import android.widget.BaseAdapter;
import android.widget.TextView;

import java.util.List;
import java.util.ArrayList;

import com.example.unitsaude.R;
import com.example.unitsaude.activities.consultation.SelectDateActivity;

public class SelectSpecialtyAdapter extends BaseAdapter {
    private Context context;
    private List<String> specialties = new ArrayList<>();
    private String selectedArea;

    public SelectSpecialtyAdapter(Context context, List<String> specialties, String selectedArea) {
        this.context = context;
        this.specialties = new ArrayList<>(specialties);
        this.selectedArea = selectedArea;
    }

    @Override
    public int getCount() {
        return specialties.size(); // CORRIGIDO
    }

    @Override
    public Object getItem(int position) {
        return specialties.get(position); // CORRIGIDO
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        if (convertView == null) {
            LayoutInflater inflater = LayoutInflater.from(context);
            convertView = inflater.inflate(R.layout.list_specialty, parent, false);
        }

        String selectedSpecialty = specialties.get(position); // CORRIGIDO

        TextView specialtyText = convertView.findViewById(R.id.specialtyText);
        specialtyText.setText(selectedSpecialty);

        convertView.setOnClickListener(v -> {
            Intent intent = new Intent(context, SelectDateActivity.class);
            intent.putExtra("selected_area", selectedArea);
            intent.putExtra("selected_specialty", selectedSpecialty);
            context.startActivity(intent);
        });

        return convertView;
    }
}
