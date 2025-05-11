package com.example.unitsaude.adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.example.unitsaude.R;
import android.widget.TextView;

import com.example.unitsaude.data.dto.consultation.GetConsultationDto;

import java.util.List;

public class ConsultaAgendadaAdapter extends ConsultaAdapter {

    public ConsultaAgendadaAdapter(Context context, List<GetConsultationDto> consultas) {
        super(context, consultas); // Chama o construtor da classe pai (ConsultaAdapter)
    }
    
    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        // Se a view (convertView) for nula, inflar um novo layout
        if (convertView == null) {
            LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            convertView = inflater.inflate(R.layout.list_consulta_agendada, parent, false);
        }

        // Obter o objeto GetConsultationDto na posição atual
        GetConsultationDto consulta = consultas.get(position);

        TextView tvArea = convertView.findViewById(R.id.tvArea);
        TextView tvData = convertView.findViewById(R.id.tvData);
        TextView tvHora = convertView.findViewById(R.id.tvHora);

        // Definir os valores nas views
        tvArea.setText(consulta.getArea());
        tvData.setText(consulta.getData());
        tvHora.setText(consulta.getHora());

        return convertView;
    }
}