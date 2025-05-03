package com.example.unitsaude.data.api;

import com.example.unitsaude.data.dto.auth.LoginRequest;
import com.example.unitsaude.data.dto.auth.LoginResponse;
import com.example.unitsaude.data.dto.auth.RecuperarSenhaRequest;
import com.example.unitsaude.data.dto.auth.RecuperarSenhaResponse;
import com.example.unitsaude.data.dto.auth.RedefinirSenhaRequest;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.POST;

public interface ApiService {

    @POST("/api/Usuario/Login")
    Call<LoginResponse> login(@Body LoginRequest loginRequest);

    @POST("/api/Usuario/recuperar-senha")
    Call<ResponseBody> recuperarSenha(@Body RecuperarSenhaRequest emailRequest);

    @POST("/api/Usuario/redefinir-senha")
    Call<ResponseBody> redefinirSenha(@Body RedefinirSenhaRequest request);


}
