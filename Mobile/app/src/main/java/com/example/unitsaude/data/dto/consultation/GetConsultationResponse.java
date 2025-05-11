package com.example.unitsaude.data.dto.consultation;

import java.util.List;

public class GetConsultationResponse {
    private List<GetConsultationDto> data; // <-- nome igual ao JSON
    private String message;
    private boolean status;

    public List<GetConsultationDto> getConsultas() {
        return data; // <-- retorna o campo correto
    }

    public void setData(List<GetConsultationDto> data) {
        this.data = data;
    }

    public String getMessage() {
        return message;
    }

    public boolean isStatus() {
        return status;
    }
}