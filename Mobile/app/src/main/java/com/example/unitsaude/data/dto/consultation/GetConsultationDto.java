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

    public String getData() {
        return this.data;
    }
    public String getHora() {
        return this.horario;
    }
    public String getEspecialidade() {
        return this.especialidade;
    }
    public String getArea() {
        return this.area;
    }
    public String getStatus() {
        return this.status;
    }
    public String getProfessorName() {
        return this.nomeProfessor;
    }

    public String[] getConsulta() {
        String[] consulta = new String[7];
        consulta[0] = this.data;
        consulta[1] = this.horario;
        consulta[2] = this.status;
        consulta[3] = this.especialidade;
        consulta[4] = this.area;
        consulta[5] = this.nomeProfessor;
        consulta[6] = this.nomePaciente;
        return consulta;
    }
}