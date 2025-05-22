package com.example.unitsaude.data.dto.consultation;

public class GetConsultationDto {
    private int id_Consulta;
    private String data;
    private String horario;
    private String status;
    private String especialidade;
    private String area;
    private int pacienteId;
    private int professorId;
    private String nomePaciente;
    private String nomeProfessor;
    private String anamnese; // Novo campo

    public int getId() {
        return this.id_Consulta;
    }
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
    public String getAnamnese() { // Novo getter
        return this.anamnese;
    }

    public String[] getConsulta() {
        String[] consulta = new String[8];
        consulta[0] = this.data;
        consulta[1] = this.horario;
        consulta[2] = this.status;
        consulta[3] = this.especialidade;
        consulta[4] = this.area;
        consulta[5] = this.nomeProfessor;
        consulta[6] = this.nomePaciente;
        consulta[7] = this.anamnese; // Adicionado anamnese
        return consulta;
    }
}