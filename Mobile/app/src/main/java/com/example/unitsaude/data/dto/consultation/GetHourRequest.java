package com.example.unitsaude.data.dto.consultation;

public class GetHourRequest {
    private String data;
    private String area;
    private String especialidade;


    public GetHourRequest(String data, String area, String especialidade) {
        this.data = data;
        this.area = area;
        this.especialidade = especialidade;
    }

    public String getData() {
        return data;
    }
    public String getArea() {
        return area;
    }
    public String getEspecialidade() {
        return especialidade;
    }
}
