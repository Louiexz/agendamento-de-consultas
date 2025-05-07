package com.example.unitsaude.data.dto.auth;

public class RedefinirSenhaRequest {
    private String token;
    private String novaSenha;

    public RedefinirSenhaRequest(String token, String novaSenha) {
        this.token = token;
        this.novaSenha = novaSenha;
    }

    // Getters e Setters (se necessário)

    public String getToken() {
        return token;
    }

    public void setToken(String token) {
        this.token = token;
    }

    public String getNovaSenha() {
        return novaSenha;
    }

    public void setNovaSenha(String novaSenha) {
        this.novaSenha = novaSenha;
    }
}
