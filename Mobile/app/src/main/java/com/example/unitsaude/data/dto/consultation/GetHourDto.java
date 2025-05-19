package com.example.unitsaude.data.dto.consultation;

public class GetHourDto {
    private String horario;
    private String status;

    public String getHora() {
        return this.horario;
    }
    public String getStatus() {
        return this.status;
    }
    public String[] getHours() {
        String[] hour = new String[2];
        hour[0] = this.horario;
        hour[1] = this.status;
        
        return hour;
    }
}