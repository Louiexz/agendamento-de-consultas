package com.example.unitsaude.data.api;

import android.content.Context;
import okhttp3.OkHttpClient;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class ApiClient {

    private static Retrofit retrofit;

    public static Retrofit getInstance() {
        if (retrofit == null) {
            retrofit = new Retrofit.Builder()
                    .baseUrl("https://agendamento-de-consultas.onrender.com") // ex: http://10.0.2.2:5000/
                    .addConverterFactory(GsonConverterFactory.create())
                    .build();
        }
        return retrofit;
    }

}