package com.example.unitsaude.utils;

import java.util.HashMap;
import java.util.Map;

public class MesesDoAno {
    public static Map<String, String> obterMapaMeses() {
        Map<String, String> meses = new HashMap<>();
        meses.put("01", "janeiro");
        meses.put("02", "fevereiro");
        meses.put("03", "março");
        meses.put("04", "abril");
        meses.put("05", "maio");
        meses.put("06", "junho");
        meses.put("07", "julho");
        meses.put("08", "agosto");
        meses.put("09", "setembro");
        meses.put("10", "outubro");
        meses.put("11", "novembro");
        meses.put("12", "dezembro");
        return meses;
    }
}
