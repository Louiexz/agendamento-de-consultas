package com.example.unitsaude.data.repositorio;

import com.example.unitsaude.data.api.ApiClient;
import com.example.unitsaude.data.api.ApiService;
import com.example.unitsaude.data.dto.professor.GetProfessorResponse;
import com.example.unitsaude.data.dto.professor.GetProfessorDto;
import retrofit2.Call;

public class ProfessorRepository {
    private final ApiService apiService;

    public ProfessorRepository() {
        this.apiService = ApiClient.getInstance().create(ApiService.class);
    }

    public Call<GetProfessorResponse> getProfessoresPorEspecialidade(String especialidade) {
        return apiService.getProfessoresPorEspecialidade(especialidade);
    }
}