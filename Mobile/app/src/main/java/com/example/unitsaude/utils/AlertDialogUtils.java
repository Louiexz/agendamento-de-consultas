package com.example.unitsaude.utils;

import android.app.AlertDialog;
import android.content.Context;

import com.example.unitsaude.R;

import android.content.Intent;
import android.app.Activity;

import android.view.View;

public class AlertDialogUtils {
    private static AlertDialog loadingDialog;
    private static AlertDialog confirmationDialog;
    private static AlertDialog cancelConsultationDialog;
    private static AlertDialog consultDetailsDialog;
    private Context context;
    private Activity activity;

    public AlertDialogUtils(Context context) {
        this.context = context;
    }

    public void closeLoadingDialog() {
        if (loadingDialog != null && loadingDialog.isShowing()) {
            loadingDialog.dismiss();
        }
    }

    public void closeConfirmationDialog() {
        if (confirmationDialog != null && confirmationDialog.isShowing()) {
            confirmationDialog.dismiss();
        }
    }

    public void showConsultConfirmationDialog(com.example.unitsaude.data.dto.consultation.GetConsultationDto consulta, String[] dateList) {
        confirmationDialog = new AlertDialog.Builder(context)
            .setTitle("Confirmação da consulta")
            .setMessage(
                "Área: " + consulta.getArea() + "\n" +
                "Especialidade: " + consulta.getEspecialidade() + "\n" +
                "Data: " + dateList[0] + " de " +
                dateList[1] + " de " + dateList[2] + "\n" +
                "Professor: " + consulta.getProfessorName() + "\n" +
                "Horário: " + consulta.getHora() + "\n" +
                "Status: " + consulta.getStatus() + "\n" +
                "Anamnese: " + consulta.getAnamnese())
            .setPositiveButton("OK", (dialog, which) -> {
                // Ação ao confirmar a consulta
                Intent intent = new Intent(context, com.example.unitsaude.MainActivity.class);
                context.startActivity(intent);
                if (context instanceof Activity) {
                    ((Activity) context).finish();  // Safely cast and finish
                }
            })
            .create();
        
        if(confirmationDialog != null && !confirmationDialog.isShowing()) {
            confirmationDialog.show();
        }
    }

    public void showLoadingDialog(String title) {
        if (title == null || title.isEmpty()) {
            title = "Carregando";
        }

        loadingDialog = new AlertDialog.Builder(context)
            .setTitle(title)
            .setView(R.layout.loading_dialog)
            .setCancelable(false)
            .create();
        
        if(loadingDialog != null && !loadingDialog.isShowing()) {
            loadingDialog.show();
        }        
    }

    public void showCancelConsultationDialog() {
        cancelConsultationDialog = new AlertDialog.Builder(context)
            .setTitle("Consulta cancelada")
            .setMessage("A consulta foi cancelada com sucesso.")
            .setPositiveButton("OK", (dialog, which) -> dialog.dismiss())
            .create();
        
        cancelConsultationDialog.setCanceledOnTouchOutside(true);
        if (cancelConsultationDialog != null && cancelConsultationDialog.isShowing()) {
            cancelConsultationDialog.show();
        }
    }

    public void showConsultDetailsDialog(com.example.unitsaude.data.dto.consultation.GetConsultationDto consulta) {
        String[] detalhes = consulta.getConsulta();

        if (detalhes == null || detalhes.length < 8) {
            new AlertDialog.Builder(context)
                .setTitle("Erro")
                .setMessage("Erro ao carregar os detalhes da consulta.")
                .setPositiveButton("OK", null)
                .show();
            return;
        }

        consultDetailsDialog = new AlertDialog.Builder(context)
            .setTitle("Detalhes da Consulta")
            .setMessage("Professor: " + detalhes[5] + "\n" +
                        "Paciente: " + detalhes[6] + "\n" +
                        "Área: " + detalhes[4] + "\n" +
                        "Especialidade: " + detalhes[3] + "\n" +
                        "Data: " + detalhes[0] + "\n" +
                        "Hora: " + detalhes[1] + "\n" +
                        "Status: " + detalhes[2] + "\n" +
                "Anamnese: " + detalhes[7])

            .setPositiveButton("OK", null)
            .create();
        if(consultDetailsDialog != null && !consultDetailsDialog.isShowing()) {
            consultDetailsDialog.show();
        }
    }
}