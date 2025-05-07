package com.example.unitsaude.data.dto.auth;

import com.google.gson.JsonObject;

public class LoginResponse {
    private String token;
    private JsonObject usuario;  // Usando JsonObject para o campo usuário

    // Getters e setters
    public String getToken() {
        return token;
    }

    public void setToken(String token) {
        this.token = token;
    }

    public JsonObject getUsuario() {
        return usuario;
    }

    public void setUsuario(JsonObject usuario) {
        this.usuario = usuario;
    }
}
