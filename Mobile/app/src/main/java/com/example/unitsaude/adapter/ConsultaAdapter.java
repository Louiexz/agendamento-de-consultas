package com.example.unitsaude.adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import android.widget.BaseAdapter;
import android.widget.TextView;
import android.app.AlertDialog;

import com.example.unitsaude.data.dto.consultation.GetConsultationDto;
import com.example.unitsaude.utils.MesesDoAno;
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

    public void showDialog(GetConsultationDto consulta) {
        String[] consultaDetails = consulta.getConsulta();
        if (consultaDetails == null || consultaDetails.length < 7) {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.setTitle("Erro");
            builder.setMessage("Erro ao carregar os detalhes da consulta.");
            builder.setPositiveButton("OK", null);
            builder.show();
            return;
        }

        AlertDialog.Builder builder = new AlertDialog.Builder(context);
        builder.setTitle("Detalhes da Consulta");

        String data = consultaDetails[0];

        String dia = data.substring(8, 10);
        String mesNumero = data.substring(5, 7);
        String mesNome = MesesDoAno.obterMapaMeses().get(mesNumero);
        String ano = data.substring(0, 4);
        
        builder.setMessage(
                "Professor: " + consultaDetails[5] + "\n" +
                "Paciente: " + consultaDetails[6] + "\n" +
                "Área: " + consultaDetails[4] + "\n" +
                "Especialidade: " + consultaDetails[3] + "\n" +                
                "Data: " + dia +
                    " de " + mesNome +
                    " de " + ano + "\n" +
                "Hora: " + consultaDetails[1] + "\n" + 
                "Status: " + consultaDetails[2]);
        builder.setPositiveButton("OK", null);
        builder.show();
    }
}