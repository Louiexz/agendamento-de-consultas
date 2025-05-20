package com.example.unitsaude.adapter;

import android.app.AlertDialog;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.recyclerview.widget.RecyclerView;

import com.example.unitsaude.R;
import com.example.unitsaude.data.dto.consultation.GetConsultationDto;

import com.example.unitsaude.utils.AlertDialogUtils;

import java.util.List;

public class ConsultaAgendadaAdapter extends RecyclerView.Adapter<ConsultaAgendadaAdapter.ViewHolder> {
    private Context context;
    private List<GetConsultationDto> consultas;
    private AlertDialogUtils alertDialogUtils;

    public ConsultaAgendadaAdapter(Context context, List<GetConsultationDto> consultas) {
        this.context = context;
        this.consultas = consultas;
        this.alertDialogUtils = new AlertDialogUtils(context);
    }

    @Override
    public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.list_consulta_agendada, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(ViewHolder holder, int position) {
        GetConsultationDto consulta = consultas.get(position);

        holder.tvArea.setText(consulta.getArea());
        holder.tvData.setText(consulta.getData());
        holder.tvHora.setText(consulta.getHora());

        holder.itemView.setOnClickListener(v -> this.alertDialogUtils.showConsultDetailsDialog(consulta));
    }

    @Override
    public int getItemCount() {
        return consultas.size();
    }

    public class ViewHolder extends RecyclerView.ViewHolder {
        TextView tvArea, tvData, tvHora;

        public ViewHolder(View itemView) {
            super(itemView);
            tvArea = itemView.findViewById(R.id.tvArea);
            tvData = itemView.findViewById(R.id.tvData);
            tvHora = itemView.findViewById(R.id.tvHora);
        }
    }
}
