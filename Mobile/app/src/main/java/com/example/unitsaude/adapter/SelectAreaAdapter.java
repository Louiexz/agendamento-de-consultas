package com.example.unitsaude.adapter;

import android.content.Context;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.BaseAdapter;
import android.widget.TextView;

import com.example.unitsaude.R;
import com.example.unitsaude.activities.consultation.SelectSpecialtyActivity;

public class SelectAreaAdapter extends BaseAdapter {
    private Context context;

    private String[] areas = {
        "Enfermagem",
        "Estética",
        "Fisioterapia",
        "Nutrição",
        "Odontologia",
        "Psicologia",
    };

    private String[] icons = {
        "ic_enfermagem",
        "ic_estetica",
        "ic_fisioterapia",
        "ic_nutricao",
        "ic_odontologia",
        "ic_psicologia",
    };

    public SelectAreaAdapter(Context context) {
        this.context = context;
    }

    @Override
    public int getCount() {
        return areas.length;
    }

    @Override
    public Object getItem(int position) {
        return areas[position];
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        if (convertView == null) {
            LayoutInflater inflater = LayoutInflater.from(context);
            convertView = inflater.inflate(R.layout.list_area, parent, false);
        }

        String selectedArea = areas[position];
        String iconName = icons[position];

        ImageView areaIcon = convertView.findViewById(R.id.areaIcon);
        int iconResId = context.getResources().getIdentifier(iconName, "drawable", context.getPackageName());
        areaIcon.setImageResource(iconResId);

        TextView areaText = convertView.findViewById(R.id.areaText);
        areaText.setText(selectedArea);

        convertView.setOnClickListener(v -> {
            Intent intent = new Intent(context, SelectSpecialtyActivity.class);
            intent.putExtra("selected_area", selectedArea);
            context.startActivity(intent);
        });

        return convertView;
    }
}
