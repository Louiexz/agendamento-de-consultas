package com.example.unitsaude.adapter;

import android.app.AlertDialog;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import android.widget.Button;
import android.widget.TextView;

import androidx.recyclerview.widget.RecyclerView;

import com.example.unitsaude.R;
import com.example.unitsaude.data.dto.consultation.GetConsultationDto;
import com.example.unitsaude.interfaces.OnCancelClickListener;

import com.example.unitsaude.utils.AlertDialogUtils;

import java.util.List;

public class ConsultaEsperaAdapter extends RecyclerView.Adapter<ConsultaEsperaAdapter.ViewHolder> {
    private Context context;
    private List<GetConsultationDto> consultas;
    private OnCancelClickListener cancelClickListener;
    private AlertDialogUtils alertDialogUtils;

    public ConsultaEsperaAdapter(Context context, List<GetConsultationDto> consultas, OnCancelClickListener cancelClickListener) {
        this.context = context;
        this.consultas = consultas;
        this.cancelClickListener = cancelClickListener;
        this.alertDialogUtils = new AlertDialogUtils(context);
    }

    public void removerConsultaPorId(int consultaId) {
        for (int i = 0; i < consultas.size(); i++) {
            if (consultas.get(i).getId() == consultaId) {
                consultas.remove(i);
                notifyItemRemoved(i);
                break;
            }
        }
    }

    @Override
    public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.list_consulta_espera, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(ViewHolder holder, int position) {
        GetConsultationDto consulta = consultas.get(position);
        holder.tvArea.setText(consulta.getArea());
        holder.tvStatus.setText(consulta.getStatus());

        holder.btnCancelar.setOnClickListener(v -> showDialogCancel(consulta.getId()));
        holder.itemView.setOnClickListener(v -> this.alertDialogUtils.showConsultDetailsDialog(consulta));
    }

    @Override
    public int getItemCount() {
        return consultas.size();
    }

    private void showDialogCancel(int consultaId) {
        new AlertDialog.Builder(context)
            .setTitle("Cancelar Consulta")
            .setMessage("Deseja cancelar a consulta?")
            .setPositiveButton("Sim", (dialog, which) -> {
                if (cancelClickListener != null) {
                    cancelClickListener.onCancelClick(consultaId);
                }
            })
            .setNegativeButton("Não", null)
            .show();
    }

    public static class ViewHolder extends RecyclerView.ViewHolder {
        TextView tvArea, tvStatus;
        Button btnCancelar;

        public ViewHolder(View itemView) {
            super(itemView);
            tvArea = itemView.findViewById(R.id.tvArea);
            tvStatus = itemView.findViewById(R.id.tvStatus);
            btnCancelar = itemView.findViewById(R.id.btnCancelar);
        }
    }
}
