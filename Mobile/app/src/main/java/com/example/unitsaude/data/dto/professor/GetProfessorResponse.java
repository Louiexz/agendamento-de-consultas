package com.example.unitsaude.data.dto.professor;

import java.util.List;
import com.example.unitsaude.data.dto.professor.GetProfessorDto;

public class GetProfessorResponse {
    private List<GetProfessorDto> data; // <-- nome igual ao JSON
    private String message;
    private boolean status;

    // Getters and Setters
    public List<GetProfessorDto> getProfessores() {
        return data; // <-- retorna o campo correto
    }
    protected void setData(List<GetProfessorDto> data) {
        this.data = data;
    }
    public String getMessage() {
        return message;
    }
    public boolean isStatus() {
        return status;
    }
}