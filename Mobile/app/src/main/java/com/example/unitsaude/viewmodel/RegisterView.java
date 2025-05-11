package com.example.unitsaude.viewmodel;

import android.app.Application;
import android.util.Log;

import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import com.example.unitsaude.data.api.ApiService;
import com.example.unitsaude.data.api.ApiClient;
import com.example.unitsaude.data.dto.auth.RegisterRequest;
import com.example.unitsaude.data.repositorio.AuthRepository;
import com.example.unitsaude.utils.SharedPreferencesManager;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import okhttp3.ResponseBody;

public class RegisterView extends AndroidViewModel {
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<ResponseBody> registerResponseLiveData = new MutableLiveData<>();
    private final AuthRepository authRepository;

    public RegisterView(Application application) {
        super(application);
        authRepository = new AuthRepository();
    }

    public LiveData<String> getErrorLiveData() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoadingLiveData() {
        return loadingLiveData;
    }

    public LiveData<ResponseBody> getRegisterResponseLiveData() {
        return registerResponseLiveData;
    }

    public void registrar(String cpf, String nome, String email, String senha,
                          String telefone, String dataNascimento) {
        loadingLiveData.setValue(true);

        authRepository.registrarPaciente(cpf, nome, email, senha,
                telefone, dataNascimento).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                loadingLiveData.setValue(false);
                if (response.isSuccessful() && response.body() != null) {
                    handleSuccessfulRegister(response.body());
                } else {
                    handleRegisterError(response);
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                loadingLiveData.setValue(false);
                errorLiveData.setValue("Erro ao conectar com o servidor");
                Log.e("REGISTER_ERROR", "Erro ao conectar com o servidor", t);
            }
        });
    }

    private void handleSuccessfulRegister(ResponseBody body) {
        registerResponseLiveData.setValue(body);
    }

    private void handleRegisterError(Response<ResponseBody> response) {
        String errorBody = "";
        try {
            if (response.errorBody() != null) {
                errorBody = response.errorBody().string();
            }
        } catch (Exception e) {
            errorBody = "Erro ao ler o corpo de erro: " + e.getMessage();
        }
        Log.e("Register_FAIL", "Código HTTP: " + response.code() + ", Erro: " + errorBody);
        errorLiveData.setValue("Erro ao registrar: " + errorBody);
    }
}
