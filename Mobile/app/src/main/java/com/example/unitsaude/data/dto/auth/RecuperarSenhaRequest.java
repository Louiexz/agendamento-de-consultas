package com.example.unitsaude.data.dto.auth;

public class RecuperarSenhaRequest {
    private String email;

    public RecuperarSenhaRequest(String email) {
        this.email = email;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }
}
