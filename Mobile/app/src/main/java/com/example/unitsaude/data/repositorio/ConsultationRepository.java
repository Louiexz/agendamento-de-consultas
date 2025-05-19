package com.example.unitsaude.data.repositorio;

import com.example.unitsaude.data.api.ApiClient;
import com.example.unitsaude.data.api.ApiService;
import com.example.unitsaude.data.dto.consultation.GetConsultationResponse;
import com.example.unitsaude.data.dto.consultation.GetSpecialtyResponse;

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
}