package com.example.unitsaude.utils;

import android.content.Context;
import android.content.SharedPreferences;

import android.content.Context;
import android.content.SharedPreferences;

public class SharedPreferencesManager {

    private static final String PREF_NAME = "UnitSaudePreferences";
    private static final String KEY_AUTH_TOKEN = "auth_token";
    private SharedPreferences sharedPreferences;

    public SharedPreferencesManager(Context context) {
        sharedPreferences = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
    }

    public void saveAuthToken(String token) {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putString(KEY_AUTH_TOKEN, token);
        editor.apply();  // Salva de forma assíncrona
    }

    public void saveUserInfo(String nome, String tipoUsuario) {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putString("user_nome", nome);
        editor.putString("user_tipo", tipoUsuario);
        editor.apply();
    }

    public String getUserNome() {
        return sharedPreferences.getString("user_nome", null);
    }

    public String getUserTipo() {
        return sharedPreferences.getString("user_tipo", null);
    }


    public String getAuthToken() {
        return sharedPreferences.getString(KEY_AUTH_TOKEN, null);
    }

    public void clearAuthToken() {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.remove(KEY_AUTH_TOKEN);
        editor.apply();
    }
}

