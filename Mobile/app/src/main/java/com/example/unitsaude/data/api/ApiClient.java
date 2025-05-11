package com.example.unitsaude.data.api;

import okhttp3.Interceptor;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

import java.io.IOException;

public class ApiClient {
    private static Retrofit retrofit;
    private static String authToken = "";

    public static void setAuthToken(String token) {
        authToken = token;
        retrofit = null; // força recriação do Retrofit com o novo token
    }

    public static ApiService getApiService() {
        return getInstance().create(ApiService.class);
    }
    public static Retrofit getInstance() {
        if (retrofit == null) {
            OkHttpClient client = new OkHttpClient.Builder()
                .addInterceptor(new Interceptor() {
                    @Override
                    public Response intercept(Chain chain) throws IOException {
                        Request original = chain.request();
                        Request.Builder builder = original.newBuilder();
                        if (!authToken.isEmpty()) {
                            builder.header("Authorization", "Bearer " + authToken);
                        }
                        Request request = builder.build();
                        return chain.proceed(request);
                    }
                })
                .build();

            retrofit = new Retrofit.Builder()
                    .baseUrl("https://agendamento-de-consultas.onrender.com") // ou sua URL de produção
                    .client(client)
                    .addConverterFactory(GsonConverterFactory.create())
                    .build();
        }
        return retrofit;
    }
}