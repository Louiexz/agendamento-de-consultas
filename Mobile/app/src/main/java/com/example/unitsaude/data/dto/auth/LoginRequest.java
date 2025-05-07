package com.example.unitsaude.data.dto.auth;

public class LoginRequest {
    private String credential;
    private String password;


    public LoginRequest(String email, String senha) {
        this.credential = email;
        this.password = senha;
    }

    // Getters e Setters
}
