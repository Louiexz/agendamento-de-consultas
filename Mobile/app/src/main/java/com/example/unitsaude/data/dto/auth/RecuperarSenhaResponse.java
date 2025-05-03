package com.example.unitsaude.data.dto.auth;

public class RecuperarSenhaResponse {
    private String message; // A mensagem de resposta

    // Construtor
    public RecuperarSenhaResponse(String message) {
        this.message = message;
    }

    // Getter e Setter
    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }
}
