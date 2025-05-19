package com.example.unitsaude.data.dto.consultation;

import java.util.List;

public class GetSpecialtyResponse {
    private List<String> data; // <-- nome igual ao JSON
    private String message;
    private boolean status;

    public List<String> getSpecialities() {
        return data; // <-- retorna o campo correto
    }

    protected void setData(List<String> data) {
        this.data = data;
    }

    public String getMessage() {
        return message;
    }

    public boolean isStatus() {
        return status;
    }
}