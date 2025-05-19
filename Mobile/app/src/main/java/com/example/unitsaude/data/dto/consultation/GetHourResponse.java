package com.example.unitsaude.data.dto.consultation;

import com.example.unitsaude.data.dto.consultation.GetHourDto;
import java.util.List;

public class GetHourResponse {
    private List<GetHourDto> data; // <-- nome igual ao JSON
    private String message;
    private boolean status;

    public List<GetHourDto> getHours() {
        return data; // <-- retorna o campo correto
    }

    protected void setData(List<GetHourDto> data) {
        this.data = data;
    }

    public String getMessage() {
        return message;
    }

    public boolean isStatus() {
        return status;
    }
}