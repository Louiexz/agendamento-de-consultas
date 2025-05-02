package com.example.unitsaude.data.api;

import okhttp3.Interceptor;
import okhttp3.Request;
import okhttp3.Response;
import java.io.IOException;

public class AuthInterceptor implements Interceptor {

    private String token; // Armazenar o token JWT

    // Constructor para receber o token
    public AuthInterceptor(String token) {
        this.token = token;
    }

    @Override
    public Response intercept(Chain chain) throws IOException {
        // Adicionando o token no cabeçalho da requisição
        Request newRequest = chain.request().newBuilder()
                .addHeader("Authorization", "Bearer " + token) // O "Bearer" é comum em APIs REST com JWT
                .build();

        return chain.proceed(newRequest);
    }
}
