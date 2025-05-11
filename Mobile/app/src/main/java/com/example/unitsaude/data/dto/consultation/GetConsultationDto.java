package com.example.unitsaude.data.dto.consultation;

public class GetConsultationDto {
    private int id;
    private String data;
    private String horario;
    private String status;
    private String especialidade;
    private String area;
    private int pacienteId;
    private int professorId;
    private String nomePaciente;
    private String nomeProfessor;

    public String getHora() {
        return this.horario;
    }

    public String getData() {
        return this.data;
    }

    public String getStatus() {
        return this.status;
    }

    public String getArea() {
        return this.area;
    }

    public String getEspecialidade() {
        return this.especialidade;
    }
}