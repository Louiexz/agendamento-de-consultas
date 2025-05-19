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

import com.example.unitsaude.interfaces.OnSpecialtyClickListener;

public class SelectSpecialtyAdapter extends BaseAdapter {
    private Context context;
    private List<String> specialties = new ArrayList<>();
    private String selectedArea;
    public OnSpecialtyClickListener listener;

    public SelectSpecialtyAdapter(Context context, List<String> specialties, String selectedArea, OnSpecialtyClickListener listener) {
        this.context = context;
        this.specialties = new ArrayList<>(specialties);
        this.selectedArea = selectedArea;
        this.listener = listener;
    }

    @Override
    public int getCount() {
        return specialties.size();
    }

    @Override
    public Object getItem(int position) {
        return specialties.get(position);
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

        String selectedSpecialty = specialties.get(position);

        TextView specialtyText = convertView.findViewById(R.id.specialtyText);
        specialtyText.setText(selectedSpecialty);

        convertView.setOnClickListener(v -> {
            if (listener != null) {
                listener.onSpecialtyClick(selectedSpecialty);
            }
        });

        return convertView;
    }
}