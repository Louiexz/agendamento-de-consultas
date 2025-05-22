package com.example.unitsaude.data.dto.consultation;

public class CreateConsultationRequest {
    private String data;
    private String horario;
    private String status;
    private String area;
    private String especialidade;
    private int pacienteId;
    private int professorId;
    private String anamnese; // Novo campo

    public CreateConsultationRequest(String data, String horario, String status, String area,
        String especialidade, int pacienteId, int professorId, String anamnese) {
        
        this.data = data;
        this.horario = horario;
        this.status = status;
        this.area = area;
        this.especialidade = especialidade;
        this.pacienteId = pacienteId;
        this.professorId = professorId;
        this.anamnese = anamnese;
    }
}