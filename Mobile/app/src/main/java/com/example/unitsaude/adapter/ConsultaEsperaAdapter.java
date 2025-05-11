package com.example.unitsaude.adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import com.example.unitsaude.R;
import com.example.unitsaude.data.dto.consultation.GetConsultationDto;

import java.util.List;

public class ConsultaEsperaAdapter extends ConsultaAdapter {

    public ConsultaEsperaAdapter(Context context, List<GetConsultationDto> consultas) {
        super(context, consultas);
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        // Se a view (convertView) for nula, inflar um novo layout
        if (convertView == null) {
            LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            convertView = inflater.inflate(R.layout.list_consulta_espera, parent, false);
        }

        // Obter o objeto GetConsultationDto na posição atual
        GetConsultationDto consulta = consultas.get(position);

        // Definir os valores nos TextViews
        TextView tvArea = convertView.findViewById(R.id.tvArea);
        TextView tvStatus = convertView.findViewById(R.id.tvStatus);

        tvArea.setText(consulta.getArea());
        tvStatus.setText(consulta.getStatus());

        return convertView;
    }
}
