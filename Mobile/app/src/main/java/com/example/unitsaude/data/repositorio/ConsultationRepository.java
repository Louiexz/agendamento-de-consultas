package com.example.unitsaude.data.repositorio;

import com.example.unitsaude.data.api.ApiClient;
import com.example.unitsaude.data.api.ApiService;
import com.example.unitsaude.data.dto.consultation.GetConsultationResponse;

import retrofit2.Call;

public class ConsultationRepository {

    private final ApiService apiService;

    public ConsultationRepository() {
        this.apiService = ApiClient.getInstance().create(ApiService.class);
    }

    public Call<GetConsultationResponse> getConsultas(int pacienteId) {
        return apiService.getConsultas(pacienteId);
    }
}