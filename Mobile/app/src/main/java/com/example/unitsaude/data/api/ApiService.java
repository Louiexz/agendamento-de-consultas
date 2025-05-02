package com.example.unitsaude.data.api;

import com.example.unitsaude.data.dto.auth.LoginRequest;
import com.example.unitsaude.data.dto.auth.LoginResponse;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.POST;

public interface ApiService {

    @POST("/api/Usuario/Login") // substitua pelo endpoint correto
    Call<LoginResponse> login(@Body LoginRequest loginRequest);

}
