package com.example.unitsaude.data.dto.consultation;

import com.example.unitsaude.data.dto.consultation.GetConsultationDto;

public class CreateConsultationResponse {
    private GetConsultationDto data; // <-- nome igual ao JSON
    private String message;
    private boolean status;

    public GetConsultationDto getConsulta() {
        return data; // <-- retorna o campo correto
    }

    protected void setData(GetConsultationDto data) {
        this.data = data;
    }

    public String getMessage() {
        return message;
    }

    public boolean isStatus() {
        return status;
    }
}