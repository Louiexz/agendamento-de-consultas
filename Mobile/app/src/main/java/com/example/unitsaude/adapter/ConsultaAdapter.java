package com.example.unitsaude.adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;
import com.example.unitsaude.data.dto.consultation.GetConsultationDto;
import java.util.List;

public class ConsultaAdapter extends BaseAdapter {
    protected Context context;
    protected List<GetConsultationDto> consultas;

    public ConsultaAdapter(Context context, List<GetConsultationDto> consultas) {
        this.context = context;
        this.consultas = consultas;
    }

    @Override
    public int getCount() {
        return consultas.size();
    }

    @Override
    public Object getItem(int position) {
        return consultas.get(position);
    }

    @Override
    public long getItemId(int position) {
        return position;
    }
    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        return convertView;
    }
}