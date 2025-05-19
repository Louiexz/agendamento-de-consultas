package com.example.unitsaude.data.dto.professor;

import java.util.List;

public class GetProfessorDto{
    private int id;
    private String cpf;
    private String nome;
    private String email;
    private String telefone;
    private String dataNascimento;
    private String area;
    private List<String> especialidades;
    private String codigoProfissional;

    // Getters and Setters
    public int getId() {
        return id;
    }
    public String getNome() {
        return nome;
    }
}