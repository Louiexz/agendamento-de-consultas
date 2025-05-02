package com.example.unitsaude.data.repositorio;

import com.example.unitsaude.data.api.ApiClient;
import com.example.unitsaude.data.api.ApiService;
import com.example.unitsaude.data.dto.auth.LoginRequest;
import com.example.unitsaude.data.dto.auth.LoginResponse;

import retrofit2.Call;

public class AuthRepository {

    private final ApiService apiService;

    public AuthRepository() {
        this.apiService = ApiClient.getInstance().create(ApiService.class);
    }

    public Call<LoginResponse> login(String credential, String password) {
        return apiService.login(new LoginRequest(credential, password));
    }
}
