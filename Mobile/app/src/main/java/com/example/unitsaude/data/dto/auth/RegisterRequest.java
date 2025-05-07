package com.example.unitsaude.data.dto.auth;

public class RegisterRequest {
    private String cpf;
    private String nome;
    private String email;
    private String senhaHash;
    private String telefone;
    private String dataNascimento;

    public RegisterRequest(String cpf, String nome, String email, String senha,
    String telefone, String dataNascimento) {
        this.cpf = cpf;
        this.nome = nome;
        this.email = email;
        this.senhaHash = senha;
        this.telefone = telefone;
        this.dataNascimento = dataNascimento;
    }
}