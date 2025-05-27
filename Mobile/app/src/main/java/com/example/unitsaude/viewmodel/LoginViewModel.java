package com.example.unitsaude.viewmodel;

import android.app.Application;
import android.util.Base64;
import android.util.Log;

import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import com.example.unitsaude.data.api.ApiService;
import com.example.unitsaude.data.api.ApiClient;
import com.example.unitsaude.data.dto.auth.LoginRequest;
import com.example.unitsaude.data.dto.auth.LoginResponse;
import com.example.unitsaude.data.repositorio.AuthRepository;
import com.example.unitsaude.utils.SharedPreferencesManager;

import org.json.JSONObject;

import java.io.IOException;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LoginViewModel extends AndroidViewModel {

    private MutableLiveData<String> tokenLiveData;
    private MutableLiveData<String> errorLiveData;
    private MutableLiveData<Boolean> loadingLiveData;
    private final MutableLiveData<LoginResponse> loginResponseLiveData;
    private final AuthRepository authRepository;

    public LoginViewModel(Application application) {
        super(application);
        tokenLiveData = new MutableLiveData<>();
        errorLiveData = new MutableLiveData<>();
        loadingLiveData = new MutableLiveData<>(false);
        loginResponseLiveData = new MutableLiveData<>();
        authRepository = new AuthRepository();
    }

    public LiveData<String> getTokenLiveData() {
        return tokenLiveData;
    }

    public void setTokenLiveData(String token) {
        tokenLiveData.setValue(token);
    }

    public LiveData<String> getErrorLiveData() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoadingLiveData() {
        return loadingLiveData;
    }

    public LiveData<LoginResponse> getLoginResponseLiveData() {
        return loginResponseLiveData;
    }

    public void login(String credential, String password, boolean saveLogin) {
        if (credential.isEmpty() || password.isEmpty()) {
            errorLiveData.setValue("Preencha todos os campos");
            return;
        }
        loadingLiveData.setValue(true);

        authRepository.login(credential, password).enqueue(new Callback<LoginResponse>() {
            @Override
            public void onResponse(Call<LoginResponse> call, Response<LoginResponse> response) {
                loadingLiveData.setValue(false);

                if (response.isSuccessful() && response.body() != null) {
                    handleSuccessfulLogin(response.body(), saveLogin);
                } else {
                    handleLoginError(response);
                }
            }

            @Override
            public void onFailure(Call<LoginResponse> call, Throwable t) {
                loadingLiveData.setValue(false);
                errorLiveData.setValue("Erro de conexão: " + t.getMessage());
                Log.e("LOGIN_ERROR", "Erro na requisição", t);
            }
        });
    }

    private void handleSuccessfulLogin(LoginResponse response, boolean saveLogin) {
        String token = response.getToken();
        if (token != null && !token.isEmpty()) {
            SharedPreferencesManager preferencesManager = new SharedPreferencesManager(getApplication());
            preferencesManager.saveAuthToken(token);
            preferencesManager.setLoginIn(saveLogin);

             // Adicione esta linha:
            com.example.unitsaude.data.api.ApiClient.setAuthToken(token);

            try {
                String[] parts = token.split("\\.");
                String payload = new String(Base64.decode(parts[1], Base64.URL_SAFE), "UTF-8");
                JSONObject payloadJson = new JSONObject(payload);

                String nome = payloadJson.getString("unique_name"); // ou "Name" dependendo do seu claim
                String tipo = payloadJson.getString("role"); // Claim de role
                int idUsuario = payloadJson.getInt("nameid"); // Claim de NameIdentifier

                preferencesManager.saveUserInfo(nome, tipo, idUsuario);

                tokenLiveData.setValue(token);
                loginResponseLiveData.setValue(response);
            } catch (Exception e) {
                Log.e("JWT_DECODE", "Erro ao decodificar token", e);
                // Se não conseguir decodificar, ainda pode continuar com o token
                tokenLiveData.setValue(token);
            }
        } else {
            errorLiveData.setValue("Token inválido ou não encontrado");
            Log.e("LOGIN_FAIL", "Token inválido ou não encontrado");
        }
    }

    private void handleLoginError(Response<LoginResponse> response) {
        String errorBody = "";
        try {
            if (response.errorBody() != null) {
                errorBody = response.errorBody().string();
            }
        } catch (Exception e) {
            errorBody = "Erro ao ler o corpo de erro: " + e.getMessage();
        }
        Log.e("LOGIN_FAIL", "Código HTTP: " + response.code() + ", Erro: " + errorBody);
        errorLiveData.setValue("Credenciais inválidas");
    }

}
