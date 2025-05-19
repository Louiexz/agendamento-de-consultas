package com.example.unitsaude.data.repositorio;

import com.example.unitsaude.data.api.ApiClient;
import com.example.unitsaude.data.api.ApiService;

import com.example.unitsaude.data.dto.consultation.GetConsultationResponse;

import com.example.unitsaude.data.dto.consultation.CreateConsultationRequest;
import com.example.unitsaude.data.dto.consultation.CreateConsultationResponse;

import com.example.unitsaude.data.dto.consultation.GetSpecialtyResponse;
import com.example.unitsaude.data.dto.consultation.GetHourRequest;
import com.example.unitsaude.data.dto.consultation.GetHourResponse;


import retrofit2.Call;

public class ConsultationRepository {

    private final ApiService apiService;

    public ConsultationRepository() {
        this.apiService = ApiClient.getInstance().create(ApiService.class);
    }

    public Call<GetConsultationResponse> getConsultas(int pacienteId) {
        return apiService.getConsultas(pacienteId);
    }

    public Call<GetSpecialtyResponse> getSpecialities(String area) {
        return apiService.getSpecialities(area);
    }

    public Call<GetHourResponse> getHours(GetHourRequest request) {
        var data = request.getData();
        var area = request.getArea();
        var especialidade = request.getEspecialidade();

        if (data == null || data.isEmpty()) {
            throw new IllegalArgumentException("Data não pode ser nula ou vazia");
        }
        if (especialidade == null || especialidade.isEmpty()) {
            throw new IllegalArgumentException("Especialidade não pode ser nula ou vazia");
        }
        if (area == null || area.isEmpty()) {
            throw new IllegalArgumentException("Área não pode ser nula ou vazia");
        }
        return apiService.getHours(data, area, especialidade);
    }

    public Call<CreateConsultationResponse> createConsulta(CreateConsultationRequest createConsultaRequest) {
        return apiService.createConsulta(createConsultaRequest);
    }
}